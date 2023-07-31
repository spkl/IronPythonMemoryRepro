import collections

def MyMethod(dummy):
    od = collections.OrderedDict()
    for i in range(100):
        od[i] = []
    
    for v in list(od.values()):
        continue

    return dummy
